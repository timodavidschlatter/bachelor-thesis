SELECT DISTINCT
    TYP || ', ' || BELASTUNG as Typ_Bel 
FROM
    BS_KANT.V_STANDORT_BG_TRIAGE al
    INNER JOIN av_ls.v_grundstueck g1    
        ON al.GEMEINDE_ID_BFS = g1.GEMEINDE_ID_BFS
WHERE
    ST_WITHIN(ST_POINTFROMTEXT('POINT(xxx yyy)',2056), g1.geom)
AND
    ST_INTERSECTS(al.geom, g1.geom)
;