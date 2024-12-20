import os
import subprocess
import re
from io import BytesIO

import numpy as np
import matplotlib.pyplot as plt
import reportlab.lib.pagesizes
from reportlab.lib.styles import getSampleStyleSheet
from reportlab.pdfgen import canvas
from reportlab.lib.pagesizes import letter, A4
from reportlab.platypus import Paragraph, Spacer, Image, SimpleDocTemplate

AB_PATH = r"k:\Programs\Apache\Apache24\bin\ab.exe"
API_PATHS = ["http://localhost:5001/api/v1/user/neighborhood", "http://localhost:5001/api/v2/authentication/is-token-valid"]
REQUEST_COUNT = 10000
CONCURRENCY_LEVEL = 500
RESULTS_DIR = "results"
PDF_REPORT = f"load_test_report_{REQUEST_COUNT}n_{CONCURRENCY_LEVEL}c.pdf"

# Создание папки для результатов
os.makedirs(RESULTS_DIR, exist_ok=True)

def run_ab_test(api_path, requests, concurrency):
    """Запускает Apache Benchmark тест."""
    cmd = [
        AB_PATH,
        f"-n {requests}",
        f"-c {concurrency}",
        api_path
    ]
    print(f"Running: {' '.join(cmd)}")
    result = subprocess.run(cmd, capture_output=True, text=True)
    if result.returncode != 0:
        print(f"Error running Apache Benchmark: {result.stderr}")
        return None
    return result.stdout

def parse_ab_output(output):
    """Парсит вывод Apache Benchmark."""
    data = {}

    # Общее количество запросов, RPS и TPR
    data["complete_requests"] = int(re.search(r"Complete requests:\s+(\d+)", output).group(1))
    data["rps"] = float(re.search(r"Requests per second:\s+([\d.]+)", output).group(1))
    data["tpr"] = float(re.search(r"Time per request:\s+([\d.]+) \[ms\]", output).group(1))

    # Данные таблицы Connection Times
    connection_times = re.search(r"Connection Times \(ms\)(.*?)Percentage", output, re.S).group(1)
    lines = connection_times.strip().split("\n")[1:]  # Пропускаем заголовок\
    columns = ["min", "mean", "stddev", "median", "max"]
    connection_data = {}
    for c in columns:
        connection_data[c] = []
    for line in lines:
        values = list(map(float, line.split()[1:]))
        for idx in range(len(columns)):
            connection_data[columns[idx]].append(values[idx])
    data["connection_times"] = connection_data

    # Перцентили
    percentiles = re.findall(r"(\d+)%\s+(\d+)", output)
    data["percentiles"] = {int(pct): int(value) for pct, value in percentiles}

    return data

def generate_combined_graph(connection_data, api_path):
    """Создает один график с использованием subplot 2x2 для всех метрик."""
    categories = ["Connect", "Processing", "Waiting", "Total"]
    fig, axes = plt.subplots(2, 2, figsize=(8, 6))
    fig.suptitle(f"Connection Times for {api_path}", fontsize=14)

    for idx, category in enumerate(categories):
        row, col = divmod(idx, 2)
        values = [connection_data[metric][idx] for metric in ["min", "mean", "stddev", "median", "max"]]
        axes[row, col].bar(["Min", "Mean", "StdDev", "Median", "Max"], values, color="skyblue", alpha=0.7)
        axes[row, col].set_title(category)
        axes[row, col].set_ylabel("Time (ms)")

    plt.tight_layout(rect=[0, 0, 1, 0.95])  # Оставляем место для заголовка
    buffer = BytesIO()
    plt.savefig(buffer, format="png")
    plt.close()
    buffer.seek(0)
    return buffer

def generate_pdf_report(results):
    """Генерирует PDF отчет."""
    doc = SimpleDocTemplate(PDF_REPORT, pagesize=A4)
    styles = getSampleStyleSheet()
    story = []

    # Заголовок отчета
    story.append(Paragraph("Load Test Report", styles["Title"]))
    story.append(Spacer(1, 12))

    for api_path, result in results.items():
        # Заголовок для API
        story.append(Paragraph(f"API Path: {api_path}", styles["Heading2"]))
        story.append(Spacer(1, 12))

        # Таблица метрик
        metrics = [
            f"Complete Requests: {result['complete_requests']}",
            f"Requests per Second: {result['rps']:.2f}",
            f"Time per Request: {result['tpr']:.2f} ms"
        ]
        for metric in metrics:
            story.append(Paragraph(metric, styles["BodyText"]))
        story.append(Spacer(1, 12))

        # Перцентили
        story.append(Paragraph("Percentiles:", styles["BodyText"]))
        for pct, value in result["percentiles"].items():
            story.append(Paragraph(f"{pct}th Percentile: {value} ms", styles["BodyText"]))
        story.append(Spacer(1, 12))

        # Добавляем график
        img_buffer = result["graph"]
        img = Image(img_buffer, width=400, height=300)
        story.append(img)
        story.append(Spacer(1, 24))

    doc.build(story)

def main():
    results = {}
    for api_path in API_PATHS:
        print(f"Testing {api_path}...")
        output = run_ab_test(api_path, REQUEST_COUNT, CONCURRENCY_LEVEL)
        if output:
            parsed_data = parse_ab_output(output)
            graph_buffer = generate_combined_graph(parsed_data["connection_times"], api_path)
            parsed_data["graph"] = graph_buffer
            results[api_path] = parsed_data
        else:
            print(f"Failed to test {api_path}")

    generate_pdf_report(results)
    print(f"PDF report generated: {PDF_REPORT}")

if __name__ == "__main__":
    main()