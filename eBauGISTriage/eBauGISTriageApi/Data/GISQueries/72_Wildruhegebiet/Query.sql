SELECT
    'Wildruhegebiet' AS WRG
FROM
    ww_wild.v_bgtriage_wildruhegebiete dv, av_ls.v_grundstueck g1  
WHERE
    ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), g1.geom)
    AND
    (
        ST_INTERSECTS(dv.geom, g1.geom)
        OR
        trunc(ST_DISTANCE(dv.geom,g1.geom)) < 100
    )
;