SELECT DISTINCT
	gewaessername, gnrgv AS gewaessernr
FROM
    wa_ow.v_eigentuemer gew 
    INNER JOIN
    (
        SELECT * FROM av_ls.v_grundstueck
        WHERE
            ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056),geom)) g1
        ON ST_INTERSECTS(gew.geom, g1.geom) 
        OR (g1.gemeinde = 'Birsfelden' AND g1.nummer in ('1423')
    )
WHERE
	gew.gewaessername = 'Rhein'
;