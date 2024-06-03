SELECT 
	'ausserhalb Bauzone' WERT 
FROM
	np_komm.v_bg_np_perimeter
WHERE
	bg_typ <> 'innerhalb ZPS -> RBG'
	AND ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), geom)
;