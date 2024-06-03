SELECT DISTINCT
    SITE_ID, PERIMETER, trunc(ST_DISTANCE(s.geom,ST_POINTFROMTEXT('POINT (xxx yyy)',2056))) as distance  
FROM
    LU_MOBIL.v_STRAHBER_POLY s, LU_MOBIL.v_MOBILFUNK_PKT m, av_ls.v_grundstueck g1  
WHERE
    m.mobilfunk_pkt_id = s.strahber_poly_id
    AND ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), g1.geom)
    AND (ST_TOUCHES(s.geom, g1.geom)
        OR ST_INTERSECTS(s.geom, g1.geom)
        OR ST_WITHIN(s.geom, g1.geom))
ORDER BY 
    distance
;