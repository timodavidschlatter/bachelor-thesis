SELECT
  zweckbestimmung, genehmigung_nr, genehmigung_datum, plan_nr
FROM
  np_komm.v_bgtriage_snp_perimeter 
WHERE
  ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), geom )
;