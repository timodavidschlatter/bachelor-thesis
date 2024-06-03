SELECT
    gwr.gwr AS GWR
FROM
    wa_ow.v_bgtriage_gewraum_prov_und_rk gwr, av_ls.v_grundstueck g1  
WHERE
    ST_WITHIN(ST_POINTFROMTEXT('POINT (xxx yyy)',2056), g1.geom)
    AND ST_INTERSECTS(gwr.geom, g1.geom)
;

