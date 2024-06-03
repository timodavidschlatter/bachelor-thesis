SELECT DISTINCT
    GW.GEWAESSERNAME, GW.BESITZER, GW.TYP, GW.VERLAUF_TYP, trunc(ST_DISTANCE(gw.geom,g1.geom)) AS distance 
FROM
    WA_OW.V_gewaessernetz gw, av_ls.v_grundstueck g1  
WHERE
    ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), g1.geom)
    AND TRUNC(ST_DISTANCE(gw.geom,g1.geom)) < 100 
    --AND GW.BESITZER LIKE 'öffentlich%'
ORDER BY 5