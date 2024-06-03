SELECT
  bg_typ
FROM
  np_komm.v_bgtriage_np_perimeter
WHERE
  ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), geom)
;