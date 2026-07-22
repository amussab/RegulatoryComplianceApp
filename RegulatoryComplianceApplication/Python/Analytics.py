import json
import sys

data = json.load(sys.stdin)

documents = data["documents"]
bills = data["bills"]

expired = sum(1 for d in documents if d["status"] == "Expired")
expiring = sum(1 for d in documents if d["status"] == "Expiring Soon")

paid = sum(1 for b in bills if b["status"] == "Paid")
pending = sum(1 for b in bills if b["status"] == "Pending")
overdue = sum(1 for b in bills if b["status"] == "Overdue")

outstanding = sum(
    b["amount"]
    for b in bills
    if b["status"] != "Paid"
)

# -------------------------
# Compliance Score
# -------------------------

overall = 100

overall -= expired * 12
overall -= expiring * 4
overall -= overdue * 8
overall -= pending * 2

overall = max(overall, 0)

# -------------------------
# Compliance Health
# -------------------------

compliance = 100

compliance -= expired * 15
compliance -= expiring * 5

compliance = max(compliance, 0)

# -------------------------
# Financial Health
# -------------------------

financial = 100

if outstanding > 50000:
    financial = 60
elif outstanding > 20000:
    financial = 75
elif outstanding > 5000:
    financial = 90

# -------------------------
# Grade
# -------------------------

if overall >= 95:
    grade = "A+"
elif overall >= 90:
    grade = "A"
elif overall >= 80:
    grade = "B"
elif overall >= 70:
    grade = "C"
elif overall >= 60:
    grade = "D"
else:
    grade = "F"

# -------------------------
# Risk
# -------------------------

if overall >= 90:
    risk = "Low"
elif overall >= 70:
    risk = "Medium"
else:
    risk = "High"

# -------------------------
# Recommendations
# -------------------------

recommendations = []

if expired:
    recommendations.append(
        f"Renew {expired} expired document(s)."
    )

if expiring:
    recommendations.append(
        f"Review {expiring} expiring document(s)."
    )

if overdue:
    recommendations.append(
        f"Pay {overdue} overdue bill(s)."
    )

if outstanding:
    recommendations.append(
        f"Outstanding amount: SAR {outstanding:,.2f}"
    )

if overall >= 90:
    recommendations.append(
        "Compliance posture is healthy. Maintain current practices."
    )

result = {
    "OverallScore": overall,
    "Grade": grade,
    "RiskLevel": risk,
    "FinancialHealth": financial,
    "ComplianceHealth": compliance,
    "OutstandingAmount": outstanding,
    "Recommendations": recommendations
}

print(json.dumps(result))