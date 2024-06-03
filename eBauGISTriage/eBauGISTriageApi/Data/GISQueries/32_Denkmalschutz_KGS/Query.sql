SELECT
    'betroffen' AS KGS
FROM
    ge_denkm.v_kgs_gebaeude kgs   
WHERE
    ST_DWITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), kgs.geom, 2.0)
;