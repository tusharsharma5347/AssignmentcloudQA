#!/bin/bash

# CloudQA Selenium Test Runner
# Runs the automated tests for the CloudQA practice form

echo "🧪 CloudQA Selenium Tests"
echo "================================"
echo ""

# Check if dotnet is available
if ! command -v dotnet &> /dev/null; then
    echo "❌ dotnet command not found!"
    echo ""
    echo "Please install .NET SDK:"
    echo "  brew install --cask dotnet-sdk"
    echo ""
    echo "Then restart your terminal and try again."
    exit 1
fi

echo "✅ .NET SDK found: $(dotnet --version)"
echo ""

echo "📦 Restoring NuGet packages..."
dotnet restore --verbosity quiet

if [ $? -ne 0 ]; then
    echo "❌ Failed to restore packages"
    exit 1
fi

echo ""
echo "🔨 Building the project..."
dotnet build --verbosity quiet

if [ $? -ne 0 ]; then
    echo "❌ Build failed"
    exit 1
fi

echo ""
echo "🧪 Running tests..."
echo "================================"
echo ""
dotnet test --logger "console;verbosity=detailed"

echo ""
if [ $? -eq 0 ]; then
    echo "✅ All tests completed successfully!"
else
    echo "❌ Some tests failed. Check the output above for details."
    exit 1
fi
