SELECT
    dv.flurname, dv.beschluss_datum
FROM
    kp_krip.v_bgtriage_l32_vg_landschaft dv, av_ls.v_grundstueck g1  
WHERE
    ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), g1.geom)
AND
    ST_INTERSECTS(dv.geom, g1.geom)
;