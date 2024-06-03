SELECT
    ikd.OBJEKTNAME, trunc(ST_DISTANCE(ikd.geom,ST_POINTFROMTEXT('POINT (xxx yyy)',2056))) as distance 
FROM
    ge_denkm.v_ikd_poly ikd  
WHERE
    ST_DISTANCE(ikd.geom,ST_POINTFROMTEXT('POINT (xxx yyy)',2056)) <1500
ORDER BY
    distance
;