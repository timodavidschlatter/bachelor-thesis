SELECT
     'Strassenlärmbelastet' AS SLB, slb.bemerkung, slb.stand_datum
FROM
    lz_immiss.v_bgtriage_strassenlaerm slb
WHERE
    ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), slb.geom)
;