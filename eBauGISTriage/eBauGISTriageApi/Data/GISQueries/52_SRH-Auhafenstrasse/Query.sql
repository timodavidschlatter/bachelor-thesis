SELECT DISTINCT
	'Zufahrt über Auhafenstrasse' as Wert
FROM av_ls.v_grundstueck g1
WHERE
  g1.gemeinde = 'Muttenz'
  AND g1.nummer in ('1328', '2975', '9317')
  AND ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), g1.geom)
;