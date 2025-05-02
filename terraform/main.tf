terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
  }
  required_version = ">= 1.0.0"
}

provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "demo" {
  name     = var.resource_group_name
  location = var.location
}

resource "azurerm_servicebus_namespace" "demo" {
  name                = var.servicebus_namespace_name
  location            = azurerm_resource_group.demo.location
  resource_group_name = azurerm_resource_group.demo.name
  sku                 = "Standard"
}

resource "azurerm_servicebus_queue" "demo" {
  name                = var.servicebus_queue_name
  resource_group_name = azurerm_resource_group.demo.name
  namespace_name      = azurerm_servicebus_namespace.demo.name
}

resource "azurerm_key_vault" "demo" {
  name                        = var.key_vault_name
  location                    = azurerm_resource_group.demo.location
  resource_group_name         = azurerm_resource_group.demo.name
  tenant_id                   = var.tenant_id
  sku_name                    = "standard"
  soft_delete_enabled         = true
  purge_protection_enabled    = false
}

resource "azurerm_storage_account" "demo" {
  name                     = var.storage_account_name
  resource_group_name      = azurerm_resource_group.demo.name
  location                 = azurerm_resource_group.demo.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

variable "resource_group_name" {}
variable "location" { default = "westeurope" }
variable "servicebus_namespace_name" {}
variable "servicebus_queue_name" {}
variable "key_vault_name" {}
variable "tenant_id" {}
variable "storage_account_name" {}

output "resource_group_name" { value = azurerm_resource_group.demo.name }
output "servicebus_namespace_name" { value = azurerm_servicebus_namespace.demo.name }
output "servicebus_queue_name" { value = azurerm_servicebus_queue.demo.name }
output "key_vault_name" { value = azurerm_key_vault.demo.name }
output "storage_account_name" { value = azurerm_storage_account.demo.name }
