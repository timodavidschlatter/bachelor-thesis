SELECT
    'Bauzonenüberprüfung' AS Wert
FROM
    np_stand.v_bgtriage_rueckzonungsflaechen dv, av_ls.v_grundstueck g1  
WHERE
    ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), g1.geom)
    AND  ST_INTERSECTS(dv.geom, g1.geom)
;