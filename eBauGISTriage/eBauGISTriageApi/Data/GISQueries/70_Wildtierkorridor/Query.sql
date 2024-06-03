SELECT
    'Wildtierkorridor' AS WTK, (ST_INTERSECTS(dv.geom, g1.geom))::TEXT AS INTERCETS, trunc(ST_DISTANCE(dv.geom,g1.geom)) AS DISTANCE 
FROM
    kp_krip.v_bgtriage_wildtierkorridor dv, av_ls.v_grundstueck g1  
WHERE
    ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), g1.geom)
    AND
    (
        ST_INTERSECTS(dv.geom, g1.geom)
        OR
        trunc(ST_DISTANCE(dv.geom,g1.geom)) < 100
    )
;