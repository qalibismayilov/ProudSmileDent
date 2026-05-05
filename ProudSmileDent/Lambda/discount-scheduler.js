import mysql from "mysql2/promise";

export const handler = async () => {
    let connection;

    try {
        connection = await mysql.createConnection({
            host: process.env.DB_HOST,
            port: Number(process.env.DB_PORT || 3306),
            user: process.env.DB_USER,
            password: process.env.DB_PASSWORD,
            database: process.env.DB_NAME
        });

        console.log("Connected to RDS MySQL");

        const [result] = await connection.execute(`
      UPDATE Services
      SET CurrentPrice = 
        CASE
          WHEN IsDiscountActive = 1
           AND NOW() BETWEEN DiscountStart AND DiscountEnd
          THEN ROUND(OriginalPrice * 0.70, 2)
          ELSE OriginalPrice
        END
    `);

        console.log("Discount scheduler executed");
        console.log("Affected rows:", result.affectedRows);

        return {
            statusCode: 200,
            body: JSON.stringify({
                message: "Discount scheduler updated Services table successfully",
                affectedRows: result.affectedRows
            })
        };
    } catch (error) {
        console.error("Lambda error:", error);

        return {
            statusCode: 500,
            body: JSON.stringify({
                message: "Discount scheduler failed",
                error: error.message
            })
        };
    } finally {
        if (connection) {
            await connection.end();
        }
    }
};