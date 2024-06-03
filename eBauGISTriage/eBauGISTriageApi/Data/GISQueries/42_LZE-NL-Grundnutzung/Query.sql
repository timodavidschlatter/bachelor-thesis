SELECT DISTINCT
    'innerhalb Grundnutzung' as Wert
    , rnp.typ_bezeichnung, rnp.zweckbestimmung, rnp.zusatzbezeichnung, rnp.kant_bezeichnung, rnp.planung_name, rnp.kant_code
FROM np_komm.v_grundzone_union rnp
WHERE
    rnp.planung_name = 'Zonenplan Siedlung'
    AND rnp.kant_code in (1600, 1700, 1520, 1512, 1511, 1510, 1492, 1491, 1490, 1472, 1470, 1430, 1413, 1330, 1320, 1310, 1300)
    AND ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), rnp.geom)
