SELECT 
    'Bahnlärmbelastet' AS BLB , blb.bemerkung, blb.stand_datum
FROM
    lz_immiss.v_bgtriage_bahnlaerm blb
WHERE
    ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), blb.geom)
;