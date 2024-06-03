SELECT DISTINCT
    ikd.OBJEKTNAME, ikd.PARZELLEN, g1.NUMMER
FROM
    ge_denkm.v_ikd_poly ikd
    INNER JOIN av_ls.v_grundstueck g1
        ON  ikd.GEMEINDE_ID_BFS = g1.GEMEINDE_ID_BFS
            AND ikd.PARZELLEN = g1.NUMMER
    , av_ls.v_grundstueck g2  
WHERE
    ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), g2.geom)
    AND ST_ASTEXT(g1.geom) <> ST_ASTEXT(g2.geom)
    AND ST_TOUCHES(g1.geom, g2.geom)
;