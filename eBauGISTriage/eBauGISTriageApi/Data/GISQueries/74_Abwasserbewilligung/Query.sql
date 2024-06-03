SELECT
    'Abwasserbewilligung SEL' AS ABWSEL, bgnummer, bewilligungsdatum, projektbezeichnung
FROM
    wa_sel.v_bgtriage_abwasserbewilligung dv, av_ls.v_grundstueck g1  
WHERE
    ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), g1.geom)
    AND
    (
        ST_INTERSECTS(dv.geom, g1.geom)
        OR
        trunc(ST_DISTANCE(dv.geom,g1.geom)) < 1
    )
;