SELECT
  typ_bezeichnung, zweckbestimmung, zusatzbezeichnung, kant_bezeichnung, planung_name, kant_code
FROM
  np_komm.v_grundzone_union
WHERE
  ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), geom)
  AND kant_code in (1490, 1491, 1492, 1500, 1510, 1511, 1512)
  AND planung_name = 'Zonenplan Siedlung';