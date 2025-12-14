# Stage 1: Build the application
FROM maven:3.8.6-eclipse-temurin-17 AS build
WORKDIR /app
COPY pom.xml .
COPY src ./src
RUN mvn clean package -DskipTests

# Stage 2: Run the application
FROM eclipse-temurin:17-jre-jammy
WORKDIR /opt/app
COPY --from=build /app/target/*.jar app.jar
ENTRYPOINT ["java", "-Dserver.address=0.0.0.0", "-jar", "app.jar"]
