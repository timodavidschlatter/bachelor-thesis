SELECT DISTINCT
	'innerhalb Bodenbedeckung' as Wert
 	, bf.art, bf.art_code
FROM np_komm.v_grundzone_union rnp
INNER JOIN av_bb.v_boflaeche bf ON ST_INTERSECTS(bf.geom, rnp.geom)
WHERE
	rnp.planung_name = 'Zonenplan Siedlung'
	AND bf.art_code in (17,21)
	AND ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), bf.geom)
;