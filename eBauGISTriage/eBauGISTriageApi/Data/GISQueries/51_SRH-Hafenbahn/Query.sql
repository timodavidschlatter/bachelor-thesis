SELECT DISTINCT
	'Gleisanlage Hafenbahn' AS Wert
FROM av_ls.v_grundstueck g1
WHERE
 ((g1.gemeinde = 'Birsfelden' AND g1.nummer in ('1984', '2265', '2264', '1477', '1872', '1329', '324', '1339', '2794', '2254', '1536', '2610', '2443'))
  OR (g1.gemeinde = 'Muttenz' AND g1.nummer in ('1328', '1329', '2975')))
  AND ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), g1.geom)
;