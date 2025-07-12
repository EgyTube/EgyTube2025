# المرحلة الأولى: بناء الصورة الأساسية لتشغيل التطبيق
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# المرحلة الثانية: بناء المشروع
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# نسخ ملف المشروع (تأكد أن اسم الملف صحيح ومطابق)
COPY EgyTube.csproj ./
RUN dotnet restore "./EgyTube.csproj"

# نسخ باقي ملفات المشروع
COPY . ./

# نشر المشروع
RUN dotnet publish "./EgyTube.csproj" -c Release -o /app/publish

# المرحلة النهائية: إعداد الصورة النهائية
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "EgyTube.dll"]
