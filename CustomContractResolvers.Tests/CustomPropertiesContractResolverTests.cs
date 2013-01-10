﻿namespace CustomContractResolvers.Tests
{
    using System.Linq;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    using Xunit;
    using Xunit.Extensions;

    public class CustomPropertiesContractResolverTests
    {
        [Fact]
        public void ByDefaultNoFieldsAreSpecified()
        {
            // Arrange
            var customPropertiesContractResolver = new CustomPropertiesContractResolver();

            // Act

            // Assert
            Assert.False(customPropertiesContractResolver.Fields.Any());
        }

        [Fact]
        public void ByDefaultNoExcludeFieldsAreSpecified()
        {
            // Arrange
            var customPropertiesContractResolver = new CustomPropertiesContractResolver();

            // Act

            // Assert
            Assert.False(customPropertiesContractResolver.ExcludeFields.Any());
        }

        [Fact]
        public void ConvertingWithFieldsAndExcludeFieldsAreEmptySerializesObjectLikeDefault()
        {
            // Arrange
            var objectToSerialize = CreateObjectToSerialize();

            // Act
            var jsonUsingDefaultSerializer = JsonConvert.SerializeObject(objectToSerialize, CreateDefaultJsonSerializerSettings());
            var jsonUsingCustomSerializer = JsonConvert.SerializeObject(objectToSerialize, CreateCustomJsonSerializerSettings(new CustomPropertiesContractResolver()));

            // Assert
            Assert.Equal(jsonUsingDefaultSerializer, jsonUsingCustomSerializer);
        }

        [Fact]
        public void ConvertingWithFieldsContainsOneFieldOnlySerializesSpecifiedField()
        {
            // Arrange
            var objectToSerialize = CreateObjectToSerialize();
            
            var customPropertiesContractResolver = new CustomPropertiesContractResolver();
            customPropertiesContractResolver.Fields.Add("Movie.Id");

            // Act
            var json = JsonConvert.SerializeObject(objectToSerialize, CreateCustomJsonSerializerSettings(customPropertiesContractResolver));

            // Assert
            Assert.Equal("{\"Id\":12}", json);
        }

        [Theory]
        [InlineData("Movie.Id")]
        [InlineData("MOVIE.Id")]
        [InlineData("MOVIE.ID")]
        [InlineData("movie.id")]
        public void ConvertingWithFieldsIsNotCaseSensitive(string field)
        {
            // Arrange
            var objectToSerialize = CreateObjectToSerialize();

            var customPropertiesContractResolver = new CustomPropertiesContractResolver();
            customPropertiesContractResolver.Fields.Add(field);

            // Act
            var json = JsonConvert.SerializeObject(objectToSerialize, CreateCustomJsonSerializerSettings(customPropertiesContractResolver));

            // Assert
            Assert.Equal("{\"Id\":12}", json);
        }

        [Fact]
        public void ConvertingWithFieldsContainsSeveralFieldsOnlySerializesSpecifiedFields()
        {
            // Arrange
            var objectToSerialize = CreateObjectToSerialize();

            var customPropertiesContractResolver = new CustomPropertiesContractResolver();
            customPropertiesContractResolver.Fields.Add("Movie.Id");
            customPropertiesContractResolver.Fields.Add("Movie.Title");

            // Act
            var json = JsonConvert.SerializeObject(objectToSerialize, CreateCustomJsonSerializerSettings(customPropertiesContractResolver));

            // Assert
            Assert.Equal("{\"Id\":12,\"Title\":\"Inception\"}", json);
        }

        [Fact]
        public void ConvertingWithFieldsContainsFieldForNestedPropertyOnlySerializesSpecifiedFields()
        {
            // Arrange
            var objectToSerialize = CreateObjectToSerialize();

            var customPropertiesContractResolver = new CustomPropertiesContractResolver();
            customPropertiesContractResolver.Fields.Add("Movie.Title");
            customPropertiesContractResolver.Fields.Add("Movie.Director");

            // Act
            var json = JsonConvert.SerializeObject(objectToSerialize, CreateCustomJsonSerializerSettings(customPropertiesContractResolver));

            // Assert
            Assert.Equal("{\"Title\":\"Inception\",\"Director\":{}}", json);
        }

        [Fact]
        public void ConvertingWithFieldsContainsFieldOfNestedPropertyOnlySerializesSpecifiedFields()
        {
            // Arrange
            var objectToSerialize = CreateObjectToSerialize();

            var customPropertiesContractResolver = new CustomPropertiesContractResolver();
            customPropertiesContractResolver.Fields.Add("Movie.Title");
            customPropertiesContractResolver.Fields.Add("Movie.Director");
            customPropertiesContractResolver.Fields.Add("Director.Name");

            // Act
            var json = JsonConvert.SerializeObject(objectToSerialize, CreateCustomJsonSerializerSettings(customPropertiesContractResolver));

            // Assert
            Assert.Equal("{\"Title\":\"Inception\",\"Director\":{\"Name\":\"Christopher Nolan\"}}", json);
        }

        [Fact]
        public void ConvertingWithFieldsWildcardAddedForPropertySerializesAllFieldsForSpecifiedProperty()
        {
            // Arrange
            var objectToSerialize = CreateObjectToSerialize();

            var customPropertiesContractResolver = new CustomPropertiesContractResolver();
            customPropertiesContractResolver.Fields.Add("Movie.*");

            // Act
            var json = JsonConvert.SerializeObject(objectToSerialize, CreateCustomJsonSerializerSettings(customPropertiesContractResolver));

            // Assert
            Assert.Equal("{\"Id\":12,\"Title\":\"Inception\",\"Year\":2010,\"Director\":{}}", json);
        }

        [Fact]
        public void ConvertingWithFieldsWildcardAddedForNestedPropertySerializesAllFieldsForSpecifiedNestedProperty()
        {
            // Arrange
            var objectToSerialize = CreateObjectToSerialize();

            var customPropertiesContractResolver = new CustomPropertiesContractResolver();
            customPropertiesContractResolver.Fields.Add("Movie.*");
            customPropertiesContractResolver.Fields.Add("Director.*");

            // Act
            var json = JsonConvert.SerializeObject(objectToSerialize, CreateCustomJsonSerializerSettings(customPropertiesContractResolver));

            // Assert
            Assert.Equal("{\"Id\":12,\"Title\":\"Inception\",\"Year\":2010,\"Director\":{\"Id\":77,\"Name\":\"Christopher Nolan\",\"Age\":42}}", json);
        }

        [Theory]
        [InlineData("Movie.Id")]
        [InlineData("MOVIE.Id")]
        [InlineData("MOVIE.ID")]
        [InlineData("movie.id")]
        public void ConvertingWithExcludeFieldsIsNotCaseSensitive(string field)
        {
            // Arrange
            var objectToSerialize = CreateObjectToSerialize();

            var customPropertiesContractResolver = new CustomPropertiesContractResolver();
            customPropertiesContractResolver.ExcludeFields.Add(field);

            // Act
            var json = JsonConvert.SerializeObject(objectToSerialize, CreateCustomJsonSerializerSettings(customPropertiesContractResolver));

            // Assert
            Assert.Equal("{\"Title\":\"Inception\",\"Year\":2010,\"Director\":{\"Id\":77,\"Name\":\"Christopher Nolan\",\"Age\":42}}", json);
        }

        [Fact]
        public void ConvertingWithExcludeFieldsContainsOneFieldDoesNotSerializeSpecifiedField()
        {
            // Arrange
            var objectToSerialize = CreateObjectToSerialize();

            var customPropertiesContractResolver = new CustomPropertiesContractResolver();
            customPropertiesContractResolver.ExcludeFields.Add("Movie.Id");

            // Act
            var json = JsonConvert.SerializeObject(objectToSerialize, CreateCustomJsonSerializerSettings(customPropertiesContractResolver));

            // Assert
            Assert.Equal("{\"Title\":\"Inception\",\"Year\":2010,\"Director\":{\"Id\":77,\"Name\":\"Christopher Nolan\",\"Age\":42}}", json);
        }

        [Fact]
        public void ConvertingWithExcludeFieldsContainsSeveralFieldsDoesNotSerializeSpecifiedFields()
        {
            // Arrange
            var objectToSerialize = CreateObjectToSerialize();

            var customPropertiesContractResolver = new CustomPropertiesContractResolver();
            customPropertiesContractResolver.ExcludeFields.Add("Movie.Id");
            customPropertiesContractResolver.ExcludeFields.Add("Movie.Title");

            // Act
            var json = JsonConvert.SerializeObject(objectToSerialize, CreateCustomJsonSerializerSettings(customPropertiesContractResolver));

            // Assert
            Assert.Equal("{\"Year\":2010,\"Director\":{\"Id\":77,\"Name\":\"Christopher Nolan\",\"Age\":42}}", json);
        }

        [Fact]
        public void ConvertingWithExcludeFieldsContainsFieldForNestedPropertyOnlySerializesSpecifiedFields()
        {
            // Arrange
            var objectToSerialize = CreateObjectToSerialize();

            var customPropertiesContractResolver = new CustomPropertiesContractResolver();
            customPropertiesContractResolver.ExcludeFields.Add("Movie.Title");
            customPropertiesContractResolver.ExcludeFields.Add("Movie.Director");

            // Act
            var json = JsonConvert.SerializeObject(objectToSerialize, CreateCustomJsonSerializerSettings(customPropertiesContractResolver));

            // Assert
            Assert.Equal("{\"Id\":12,\"Year\":2010}", json);
        }

        [Fact]
        public void ConvertingWithExcludeFieldsContainsFieldOfNestedPropertyOnlySerializesSpecifiedFields()
        {
            // Arrange
            var objectToSerialize = CreateObjectToSerialize();

            var customPropertiesContractResolver = new CustomPropertiesContractResolver();
            customPropertiesContractResolver.ExcludeFields.Add("Movie.Title");
            customPropertiesContractResolver.ExcludeFields.Add("Director.Name");

            // Act
            var json = JsonConvert.SerializeObject(objectToSerialize, CreateCustomJsonSerializerSettings(customPropertiesContractResolver));

            // Assert
            Assert.Equal("{\"Id\":12,\"Year\":2010,\"Director\":{\"Id\":77,\"Age\":42}}", json);
        }

        [Fact]
        public void ConvertingWithExcludeFieldsWildcardAddedForPropertySerializesAllFieldsForSpecifiedProperty()
        {
            // Arrange
            var objectToSerialize = CreateObjectToSerialize();

            var customPropertiesContractResolver = new CustomPropertiesContractResolver();
            customPropertiesContractResolver.ExcludeFields.Add("Movie.*");

            // Act
            var json = JsonConvert.SerializeObject(objectToSerialize, CreateCustomJsonSerializerSettings(customPropertiesContractResolver));

            // Assert
            Assert.Equal("{}", json);
        }

        [Fact]
        public void ConvertingWithExcludeFieldsWildcardAddedForNestedPropertySerializesAllFieldsForSpecifiedNestedProperty()
        {
            // Arrange
            var objectToSerialize = CreateObjectToSerialize();

            var customPropertiesContractResolver = new CustomPropertiesContractResolver();
            customPropertiesContractResolver.ExcludeFields.Add("Director.*");

            // Act
            var json = JsonConvert.SerializeObject(objectToSerialize, CreateCustomJsonSerializerSettings(customPropertiesContractResolver));

            // Assert
            Assert.Equal("{\"Id\":12,\"Title\":\"Inception\",\"Year\":2010,\"Director\":{}}", json);
        }

        [Fact]
        public void ConvertingWithPropertyBeingSpecifiedAsBothFieldAndExcludeFieldWillNotSerializeProperty()
        {
            // Arrange
            var objectToSerialize = CreateObjectToSerialize();

            var customPropertiesContractResolver = new CustomPropertiesContractResolver();
            customPropertiesContractResolver.Fields.Add("Movie.Director");
            customPropertiesContractResolver.ExcludeFields.Add("Movie.Director");

            // Act
            var json = JsonConvert.SerializeObject(objectToSerialize, CreateCustomJsonSerializerSettings(customPropertiesContractResolver));

            // Assert
            Assert.Equal("{}}", json);
        }

        private static JsonSerializerSettings CreateCustomJsonSerializerSettings(IContractResolver contractResolver)
        {
            return new JsonSerializerSettings { ContractResolver = contractResolver };
        }

        private static JsonSerializerSettings CreateDefaultJsonSerializerSettings()
        {
            return new JsonSerializerSettings();
        }

        private static Movie CreateObjectToSerialize()
        {
            return new Movie
                       {
                           Id = 12,
                           Title = "Inception",
                           Year = 2010,
                           Director = new Director
                                          {
                                              Id = 77,
                                              Name = "Christopher Nolan",
                                              Age = 42
                                          }
                       };
        }
    }
}