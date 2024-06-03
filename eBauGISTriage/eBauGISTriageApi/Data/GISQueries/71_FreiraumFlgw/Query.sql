SELECT
    'Freiraum Fliessgewässer' AS FFGW,
    dv.flurname,
    dv.art
FROM
    kp_krip.v_bgtriage_l12_rb_gewaesser dv  
WHERE
    ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), dv.geom)
;