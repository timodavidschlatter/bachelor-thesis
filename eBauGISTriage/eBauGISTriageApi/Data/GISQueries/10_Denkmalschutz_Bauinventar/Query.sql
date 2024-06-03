SELECT 
    bib.objekttyp
FROM 
    ge_denkm.v_bib_poly bib
    INNER JOIN av_ls.v_grundstueck g1 
        ON bib.GEMEINDE_ID_BFS = g1.GEMEINDE_ID_BFS
WHERE 
    ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), g1.geom)
    AND (ST_WITHIN(bib.geom, g1.geom)
        OR ST_OVERLAPS(bib.geom, g1.geom))
;