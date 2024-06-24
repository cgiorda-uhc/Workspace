import pandas
import pyodbc


def get_data_table(str_connection_string="", str_sql=""):
    sql_conn = pyodbc.connect(str_connection_string)
    return pandas.read_sql(str_sql, sql_conn)

