SELECT 
	'grenzt an Bodenbedeckung' as Wert
 	, bf.art, bf.art_code
FROM
	av_ls.v_grundstueck g1
INNER JOIN av_bb.v_boflaeche bf ON ST_INTERSECTS(g1.geom, bf.geom) OR ST_TOUCHES(g1.geom, bf.geom)
WHERE 
	bf.art_code in (17,21)
	AND ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), g1.geom)
	AND EXISTS (SELECT 1 FROM np_komm.v_grundzone_union rnp WHERE 
		rnp.planung_name = 'Zonenplan Siedlung'
		AND ST_INTERSECTS(g1.geom, rnp.geom))		
	AND NOT ST_INTERSECTS(ST_BUFFER(g1.geom, -1), bf.geom)
;