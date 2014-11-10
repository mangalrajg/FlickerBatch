﻿update remote_DATA set DATE_TAKEN=( SELECT 
  SUBSTR(DATE_TAKEN, s2+1, 4)||
  CASE 
        WHEN SUBSTR(DATE_TAKEN, s1+1, 3)='Jan' THEN '01'
        WHEN SUBSTR(DATE_TAKEN, s1+1, 3)='Feb' THEN '02'
        WHEN SUBSTR(DATE_TAKEN, s1+1, 3)='Mar' THEN '03'
        WHEN SUBSTR(DATE_TAKEN, s1+1, 3)='Apr' THEN '04'
        WHEN SUBSTR(DATE_TAKEN, s1+1, 3)='May' THEN '05'
        WHEN SUBSTR(DATE_TAKEN, s1+1, 3)='Jun' THEN '06'
        WHEN SUBSTR(DATE_TAKEN, s1+1, 3)='Jul' THEN '07'
        WHEN SUBSTR(DATE_TAKEN, s1+1, 3)='Aug' THEN '08'
        WHEN SUBSTR(DATE_TAKEN, s1+1, 3)='Sep' THEN '09'
        WHEN SUBSTR(DATE_TAKEN, s1+1, 3)='Oct' THEN '10'
        WHEN SUBSTR(DATE_TAKEN, s1+1, 3)='Nov' THEN '11'
        WHEN SUBSTR(DATE_TAKEN, s1+1, 3)='Dec' THEN '12'
        END ||
  SUBSTR(DATE_TAKEN, 1, s1-1) ||
  CASE 
	WHEN SUBSTR(DATE_TAKEN, s3+1, s4-s3-1)='1' then '01'
	WHEN SUBSTR(DATE_TAKEN, s3+1, s4-s3-1)='2' then '02'
	WHEN SUBSTR(DATE_TAKEN, s3+1, s4-s3-1)='3' then '03'
	WHEN SUBSTR(DATE_TAKEN, s3+1, s4-s3-1)='4' then '04'
	WHEN SUBSTR(DATE_TAKEN, s3+1, s4-s3-1)='5' then '05'
	WHEN SUBSTR(DATE_TAKEN, s3+1, s4-s3-1)='6' then '06'
	WHEN SUBSTR(DATE_TAKEN, s3+1, s4-s3-1)='7' then '07'
	WHEN SUBSTR(DATE_TAKEN, s3+1, s4-s3-1)='8' then '08'
	WHEN SUBSTR(DATE_TAKEN, s3+1, s4-s3-1)='9' then '09'
	ELSE SUBSTR(DATE_TAKEN, s3+1, s4-s3-1)
	END ||
  SUBSTR(DATE_TAKEN, s4+1, s5-s4-1)||
  SUBSTR(DATE_TAKEN, s5+1, s6-s5-1) +
  CASE WHEN SUBSTR(DATE_TAKEN, s3+1, s4-s3-1) = '12' THEN '-120000' ELSE '0' END +
  CASE WHEN SUBSTR(DATE_TAKEN, s6+1) LIKE 'PM' THEN '120000' ELSE '0' END 
 FROM
(
  SELECT *,
    INSTR(SUBSTR(DATE_TAKEN, s1+1), '-')+s1 AS s2,
    INSTR(SUBSTR(DATE_TAKEN, s4+1), ':')+s4 AS s5,
    INSTR(SUBSTR(DATE_TAKEN, s3+1), ' ')+s3 AS s6
  FROM (
    SELECT DATE_TAKEN,
      INSTR(DATE_TAKEN, '-') AS s1, 
      INSTR(DATE_TAKEN, ' ') AS s3, 
      INSTR(DATE_TAKEN, ':') AS s4 
  ))
);
