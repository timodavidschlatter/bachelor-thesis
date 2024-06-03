SELECT
    CASE WHEN ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), obs.geom)
        THEN 'innerhalb'
        ELSE 'angrenzend'
    END
FROM
    ge_denkm.v_ortsbildschutz obs
    INNER JOIN av_ls.v_grundstueck g1
        ON obs.GEMEINDE_ID_BFS = g1.GEMEINDE_ID_BFS
WHERE 
    ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), g1.geom)
    AND (ST_INTERSECTS(obs.geom, g1.geom)
        OR ST_WITHIN(obs.geom, g1.geom))
;