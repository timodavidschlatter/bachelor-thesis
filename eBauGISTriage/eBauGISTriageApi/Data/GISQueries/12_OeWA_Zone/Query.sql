SELECT
  oewa_zone as OeWA_Zone 
FROM
  np_komm.v_bgtriage_grundzone_oewa rnp    
WHERE
  ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), rnp.geom) = true
