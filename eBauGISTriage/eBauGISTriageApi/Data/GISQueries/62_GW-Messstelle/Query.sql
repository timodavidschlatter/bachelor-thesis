SELECT DISTINCT
    ms.AUFSCHLUSSNR  
FROM
    WA_HYDJ.v_bgtriage_gwmessst ms
    INNER JOIN av_ls.v_grundstueck g1
        ON ST_WITHIN(ms.geom, ST_BUFFER(g1.geom, 5))
WHERE
    ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)', 2056), g1.geom)
;