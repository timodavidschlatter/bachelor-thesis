SELECT 
	'grenzt an ZPL' as Wert
FROM
	av_ls.v_grundstueck g1
INNER JOIN np_komm.v_grundzone_union rnp
	ON ST_INTERSECTS(ST_BUFFER(g1.geom, .1), rnp.geom)
WHERE 
	ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), g1.geom)
	AND rnp.planung_name = 'Zonenplan Landschaft'
	AND NOT ST_INTERSECTS(ST_BUFFER(g1.geom, -1), rnp.geom)
;