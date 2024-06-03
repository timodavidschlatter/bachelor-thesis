SELECT
  kant_bezeichnung, typ_bezeichnung, planinhalt 
FROM
  np_komm.v_grundzone_union
WHERE
  ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), geom)= true
;