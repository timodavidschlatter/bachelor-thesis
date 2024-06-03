SELECT DISTINCT
    bo.KLNR  
FROM
    WA_GW.v_bgtriage_bohrung bo
    INNER JOIN av_ls.v_grundstueck g1
        ON ST_WITHIN(bo.geom, ST_BUFFER(g1.geom, 50))
WHERE
    ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)', 2056), g1.geom)
    AND Upper(bo.mit_rohr) = 'JA'
UNION
SELECT DISTINCT
    qu.KLNR  
FROM
    WA_GW.v_bgtriage_quelle_alle qu
INNER JOIN av_ls.v_grundstueck g1
    ON ST_WITHIN(qu.geom, ST_BUFFER(g1.geom, 50))
WHERE
    ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)', 2056), g1.geom)
ORDER BY KLNR
;