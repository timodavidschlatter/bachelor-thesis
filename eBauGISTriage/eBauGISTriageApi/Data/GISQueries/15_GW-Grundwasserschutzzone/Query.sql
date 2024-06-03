SELECT DISTINCT
    ws.ART_TEXT, ws.NAMESCHUTZZONE 
FROM 
    WA_PGS.V_BGTRIAGE_GWSZONE_RRB ws
    INNER JOIN av_ls.v_grundstueck g1
        ON ST_INTERSECTS(g1.geom, ws.geom)
WHERE
    ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), g1.geom)
;