SELECT
    WAERMEVERBUND
FROM
    EN_WAERM.v_bgtriage_waermeverbund
WHERE
    ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), geom)
;