DROP PROCEDURE IF EXISTS SP_INSERT_TABLEORDER;
DELIMITER //
CREATE PROCEDURE SP_INSERT_TABLEORDER(
    -- ## 새로운 주문을 생성한다.
    IN USR_ID VARCHAR(15),
    IN SEAT_NO INT
)
BEGIN
    INSERT INTO 
        tableorder(seat_no, usr_id)
    VALUES
        (SEAT_NO, USR_ID);

    SELECT max(order_no) order_no FROM tableorder WHERE seat_no=SEAT_NO AND usr_id=USR_ID;

END //
DELIMITER ;



DROP PROCEDURE IF EXISTS SP_INSERT_SALE;
DELIMITER //
CREATE PROCEDURE SP_INSERT_SALE(
    -- ## 해당 프로시저를 사용하기 전에 반드시 TALBLEORDER를 생성할 것 ! ## 
    IN SEAT_NO INT,
    IN ORDER_NO INT,    
    IN PRO_CODE INT,
    IN SALE_COUNT INT,
    IN SALE_DISCOUNT INT
    -- sale_totprc와 sale_date는 또 다른 프로시저를 통해 자동으로 값이 기입된다.
    -- sale은 해당 테이블의 가장 높은 값을 갖는 order에 기입이 된다.
)
BEGIN
    INSERT INTO 
        sale(order_no, pro_code, sale_count, sale_discount)
    VALUES
        ((SELECT MAX(ORDER_NO) FROM tableorder WHERE seat_no = SEAT_NO), 
        PRO_CODE, SALE_COUNT, SALE_DISCOUNT);
END //
DELIMITER ;



DROP PROCEDURE IF EXISTS SP_DELETE_SALE;
DELIMITER //
CREATE PROCEDURE SP_DELETE_SALE(
    -- ## sale을 삭제하는 프로시저, 삭제 후 tableorder가 비게 되면
    -- ## tableorder 또한 삭제한다 !!!
    IN ORDERNO INT,    
    IN PROCODE INT
)
BEGIN
    DELETE FROM 
        sale 
    WHERE 
        order_no = ORDERNO AND pro_code = PROCODE;

    DELETE FROM 
        tableorder 
    WHERE 
        (order_no = ORDERNO 
        AND (SELECT COUNT(*) FROM sale WHERE order_no = ORDERNO) <= 0);    
END //
DELIMITER ;




DROP PROCEDURE IF EXISTS SP_UPDATE_SALE;
DELIMITER //
CREATE PROCEDURE SP_UPDATE_SALE(
    -- ## SALE_TOTPRC는 프로시저 내에서 연산하여 기입한다.
    IN ORDERNO INT,    
    IN PROCODE INT,
    IN SALECOUNT INT,
    IN SALEDISCOUNT INT
)
BEGIN
    UPDATE 
        sale
    SET
        sale_count = SALECOUNT, 
        sale_discount = NVL(SALEDISCOUNT, 0),
        sale_totprc = SALECOUNT * (SELECT pro_price FROM product WHERE pro_code=PROCODE),
        order_date = NOW()
    WHERE
        order_no = ORDERNO AND pro_code = PROCODE;
END //
DELIMITER ;
