SELECT
    blno.objekt_nr, blno.objekt_name, blno.version
FROM
    nl_nso.v_bln_objekte blno, av_ls.v_grundstueck g1  
WHERE
    ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), g1.geom)
    AND ST_INTERSECTS(blno.geom, g1.geom)
;