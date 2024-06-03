SELECT DISTINCT
	zweckbestimmung, plan_nr
FROM
	np_komm.v_bgtriage_snp_perimeter gnu
	INNER JOIN av_ls.v_grundstueck g1 ON
		ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), g1.geom)
		AND ST_INTERSECTS(gnu.geom, ST_BUFFER(g1.geom, -0.5))
WHERE 
	zweckbestimmung like 'KNP Rheinhäfen%'
   ;