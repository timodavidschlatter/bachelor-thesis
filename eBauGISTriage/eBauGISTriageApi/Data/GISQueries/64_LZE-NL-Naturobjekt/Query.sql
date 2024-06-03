SELECT DISTINCT
	'beinhaltet', nat.bez_kommunal, nat.zusatzbezeichnung, nat.kantonale_bezeichnung
FROM av_ls.v_grundstueck g1 
	INNER JOIN np_komm.v_bgtriage_nl_lze nat ON ST_WITHIN(nat.geom, ST_BUFFER(g1.geom, -0.5))
WHERE
	ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), g1.geom)
;