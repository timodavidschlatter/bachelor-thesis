SELECT
  typ_bezeichnung, zweckbestimmung, zusatzbezeichnung, kant_bezeichnung, planung_name, kant_code
FROM
  np_komm.v_bgtriage_grundzone_union rnp
WHERE
  ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), rnp.geom)
  AND (
    rnp.kant_bezeichnung = 'Landwirtschaftszone'
  or
    rnp.planung_name = 'Zonenplan Landschaft'
  )
  AND rnp.kant_code in (1110, 1120, 1200, 1211, 1212, 1213, 1300, 1310, 1320, 1330, 1490, 1491, 1492, 1500, 1510, 1511, 1512, 1900) 
;