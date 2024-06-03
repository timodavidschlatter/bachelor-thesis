SELECT
	'innerhalb ZPL' as Wert, typ_bezeichnung, zweckbestimmung, zusatzbezeichnung, kant_bezeichnung, planung_name, kant_code
FROM
	np_komm.v_grundzone_union
WHERE
	(code_hn >= 21 and code_hn <= 29)
	 AND kant_code <> 1492
	 AND ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), geom);