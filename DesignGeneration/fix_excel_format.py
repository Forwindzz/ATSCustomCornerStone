import win32com.client as win32
import os

def repair_excel_file(input_path, output_path=None):
    input_path = os.path.abspath(input_path)
    if output_path is not None:
        output_path = os.path.abspath(output_path)

    excel = win32.gencache.EnsureDispatch("Excel.Application")
    excel.Visible = False 
    excel.DisplayAlerts = False
    workbook = None
    try:
        workbook = excel.Workbooks.Open(input_path)

        workbook.SaveAs(output_path, FileFormat=51)  # FileFormat=51 Excel (.xlsx)
        print(f"save file to: {output_path}")

    except Exception as e:
        print(f"Error while processing file: {e}")
    finally:
        if workbook is not None:
            workbook.Close(SaveChanges=False)
        excel.Application.Quit()

input_file = "./input/⑨の自定义基石表_mod.xlsx"
output_file = "./input/⑨の自定义基石表_mod.xlsx" 
repair_excel_file(input_file, output_file)
