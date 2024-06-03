SELECT
    'Schiesslärmbelastet' AS BEL, bgv.bemerkung,bgv.stand_datum
FROM
    lz_immiss.v_bgtriage_schiesslaerm bgv
WHERE
    ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), bgv.geom)
;