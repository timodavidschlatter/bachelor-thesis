SELECT
  trunc(ST_DISTANCE(ikd.geom,ST_POINTFROMTEXT('POINT (xxx yyy)',2056))) as distance 
FROM
  ge_denkm.v_ikd_poly ikd, np_komm.v_bgtriage_np_perimeter rnp
WHERE
  rnp.bg_typ <> 'innerhalb ZPS -> RBG'
  AND ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), rnp.geom)
  AND trunc(ST_DISTANCE(ikd.geom,ST_POINTFROMTEXT('POINT (xxx yyy)',2056))) < 650
ORDER BY
  distance
;