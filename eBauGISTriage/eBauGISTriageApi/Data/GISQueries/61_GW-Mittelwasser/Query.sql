SELECT
    max(grid_code_legend) || ' m ü.M.' as hoehe
FROM
    wa_gw.v_bgtriage_mq_final_attr mq
    INNER JOIN av_ls.v_grundstueck g1
        ON ST_INTERSECTS(g1.geom, mq.geom)
WHERE
    ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056),g1.geom)
HAVING
    max(grid_code_legend) IS NOT NULL
;