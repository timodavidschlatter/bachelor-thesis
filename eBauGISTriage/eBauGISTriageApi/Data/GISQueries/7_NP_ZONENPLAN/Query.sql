SELECT
  np.rechtsstatus, np.genehmigung_nr, np.genehmigung_datum, np.plan_nr
FROM 
  np_komm.v_np_perimeter np  
WHERE
  ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), np.GEOM )= true
;
