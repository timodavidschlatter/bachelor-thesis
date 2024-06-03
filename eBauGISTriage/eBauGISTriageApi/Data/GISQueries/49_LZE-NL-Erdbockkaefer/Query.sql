SELECT 
	'Erdbockkäfer' as Wert
FROM
	av_ls.v_grundstueck g1
WHERE 
	ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), g1.geom)
	AND nummer = 'C-1320'
	AND gemeinde_id_bfs = 2762
;