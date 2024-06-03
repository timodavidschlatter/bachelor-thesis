SELECT
    stv.FIRMA, trunc(ST_DISTANCE(stv.geom,ST_POINTFROMTEXT('POINT (xxx yyy)',2056))) as distance 
FROM
    si_crisk.V_STOERFALLBETRIEB_BG_TRIAGE stv 
WHERE
    ST_DISTANCE(stv.geom,ST_POINTFROMTEXT('POINT (xxx yyy)',2056)) < 1500 
ORDER BY
    distance
