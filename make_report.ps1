rm -r test/allure-results
dotnet test --no-restore
cp -r .\test\allure-report\history\ test/allure-results/history
allure generate test/allure-results -o test/allure-report --clean
allure open test/allure-report