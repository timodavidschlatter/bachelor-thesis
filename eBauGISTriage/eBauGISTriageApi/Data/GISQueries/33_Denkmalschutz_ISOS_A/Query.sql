SELECT
    'betroffen' as ISOSA
FROM
    ge_denkm.v_bgtriage_isos isos   
WHERE
    ST_DWITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), isos.geom, 2.0)
;